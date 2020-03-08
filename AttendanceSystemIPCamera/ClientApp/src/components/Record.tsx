import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';
import { recordActionCreators } from '../store/record/recordActionCreators';
import { RecordState } from '../store/record/recordState';
import { unitActionCreators } from '../store/unit/actionCreators';
import { UnitsState } from '../store/unit/state';
import * as Moment from 'moment';
import { extendMoment } from 'moment-range';
import { constants } from '../constant';
import Attendee from '../models/Attendee';
import RecordSearch from '../models/RecordSearch';
import { Spin, Button, Icon } from 'antd';

import "react-big-calendar/lib/css/react-big-calendar.css";

import { AttendanceTable, AttendanceColumn } from './AttendanceTable';
import AttendanceTableRow from '../models/AttendanceTableRow';
import AttendanceButtonGroup from './AttendanceButtonGroup';
import { Dictionary } from '../models/Dictionary';
import Record from '../models/Record';
import Unit from '../models/Unit';
import { stringify } from 'querystring';

// At runtime, Redux will merge together...
type RecordProps =
    RecordState // ... state we've requested from the Redux store
    & UnitsState
    & typeof unitActionCreators
    & typeof recordActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

interface RecordComponentState {
    attendeeId: number;
    endDate: Date;
    startDate: Date;
    showDate: Date
}

const moment = extendMoment(Moment);

class RecordComp extends React.PureComponent<RecordProps, RecordComponentState> {

    allowComponentDidUpdateRun = false
    today = new Date();


    constructor(props: RecordProps) {
        super(props);
        let attendeeStr = localStorage.getItem(constants.AUTH_IN_LOCAL_STORAGE);
        let attendee: Attendee = JSON.parse(attendeeStr ? attendeeStr : "");

        this.state = {
            attendeeId: attendee.id,
            startDate: moment(this.today).startOf('week').toDate(),
            endDate: moment(this.today).endOf('week').toDate(),
            showDate: this.today
        };
    }

    componentDidMount() {
        this.props.requestUnits();
        this.ensureDataFetched();
    }

    componentDidUpdate() {
        if (this.allowComponentDidUpdateRun) {
            this.ensureDataFetched();
        }
        this.allowComponentDidUpdateRun = false;
    }

    public render() {
        return (
            (this.props.isLoading) ? <Spin /> :
                <>
                    <Button.Group>
                        <Button type="primary" onClick={(e) => this.handleRefresh()}>
                            <Icon type="sync" />
                            Refresh
                        </Button>
                    </Button.Group>
                    <AttendanceButtonGroup onNavigate={this.onNavigate} today={this.state.showDate} />
                    <AttendanceTable bordered dataSource={this.mapToDataSource()} pagination={false}>
                        {this.renderColumns()}
                    </AttendanceTable>
                </>

        );
    }

    onNavigate = (date: Date) => {
        this.allowComponentDidUpdateRun = true;
        console.log(date);
        const start = moment(date).startOf('week').toDate();
        const end = moment(date).endOf('week').toDate();
        this.setState({ startDate: start, endDate: end, showDate: date });
    }


    private handleRefresh() {
        var recordSearch: RecordSearch = this.getRecordSearch();
        this.props.requestRefresh(recordSearch);
    }

    private ensureDataFetched() {
        var recordSearch: RecordSearch = this.getRecordSearch();
        this.props.requestRecords(recordSearch);
    }

    private getRecordSearch() {
        return {
            attendeeId: this.state.attendeeId,
            startTime: this.state.startDate,
            endTime: this.state.endDate
        };
    }

    private renderColumns() {
        const headers = this.getHeaders();
        return headers.map(header => {
            const key = header.id; //monday or tuesday or ... or sunday
            const title = header.name; // date in string
            return <AttendanceColumn
                key={key}
                title={title}
                dataIndex={key}
                render={(text, record) => this.renderColor(text, record)} />;
        })
    }

    private getHeaders() {
        const headers = [];
        headers.push({
            id: "unit",
            name: ""
        });
        const { startDate, endDate } = this.state;
        const range = moment().range(startDate, endDate);
        const dates = Array.from(range.by("day"));
        headers.push(...dates.map((date: Moment.Moment) => {
            return {
                id: date.format("dddd").toLowerCase(), //monday or tuesday or ... or sunday
                name: date.format("ddd, DD MMM YYYY")  // date in string
            }
        }));
        return headers;
    }

    private mapToDataSource() {
        const tranformedRecords = this.tranformRecords();

        return this.props.units.map<AttendanceTableRow>((unit: Unit, index) => {
            const unitId = unit.id;
            // console.log(unit);
            return {
                key: index.toString(),
                unit: this.getUnitLabel(unit), // unit label
                monday: tranformedRecords[unitId] ? tranformedRecords[unitId]["monday"] : "",
                tuesday: tranformedRecords[unitId] ? tranformedRecords[unitId]["tuesday"] : "",
                wednesday: tranformedRecords[unitId] ? tranformedRecords[unitId]["wednesday"] : "",
                thursday: tranformedRecords[unitId] ? tranformedRecords[unitId]["thursday"] : "",
                friday: tranformedRecords[unitId] ? tranformedRecords[unitId]["friday"] : "",
                saturday: tranformedRecords[unitId] ? tranformedRecords[unitId]["saturday"] : "",
                sunday: tranformedRecords[unitId] ? tranformedRecords[unitId]["sunday"] : "",
            }
        });

    }

    private tranformRecords() { //records["1"]["monday"] // slot 1, monday
        const records: Dictionary<Dictionary<string>> = {};
        this.props.recordData.forEach((r, index) => {
            const unit = this.props.units.find(u => u.name == r.name);
            if (unit) {
                if (!records[unit.id]) {
                    records[unit.id] = {};
                }
                const dayInWeek = moment(r.startTime).format("dddd").toLowerCase();
                records[unit.id][dayInWeek] = r.groupCode + ' - ' + (r.present ? 'Present' : 'Absent');
            }
        });
        return records;
    }

    private renderColor(text: string, row: AttendanceTableRow) {
        console.log();
        if (text) {
            return {
                props: {
                    style: { background: text.includes("Present") ? "green" : text.includes('Absent') ? "red" : "" }
                },
                children: <div>{text}</div>
            };
        }
    }

    private getUnitLabel(unit: Unit) {
        return unit.name + " (" + moment(unit.startTime).format("HH:mm") + " - " + moment(unit.endTime).format("HH:mm") + ")";
    }
}


const mapDispatchToProps = (dispatch: any) => {
    return bindActionCreators({ ...recordActionCreators, ...unitActionCreators }, dispatch);
}

const mapStateToProps = (state: ApplicationState) => {
    return { ...state.record, ...state.units };
}

export default connect(mapStateToProps, mapDispatchToProps)(RecordComp);
