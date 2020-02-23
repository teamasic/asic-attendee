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
import { Spin } from 'antd';

import "react-big-calendar/lib/css/react-big-calendar.css";

import AttendanceTable from './AttendanceTable';
import AttendanceButtonGroup from './AttendanceButtonGroup';

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

class Record extends React.PureComponent<RecordProps, RecordComponentState> {

    allowComponentDidUpdateRun = false
    today = new Date(2020, 1, 20);

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
                    <AttendanceButtonGroup onNavigate={this.onNavigate} today={this.state.showDate} />
                    <AttendanceTable units={this.mapToUnits()} columns={this.mapToColumns()} events={this.mapToEvents()} />
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

    private mapToUnits() {
        return this.props.units;
    }

    private mapToEvents() {
        return this.props.recordData.map((re, i) => {

            let unit = this.props.units.filter(u => u.name === re.name)[0];
            let event = {
                id: re.id,
                title: re.groupCode + ' - ' + (re.present ? 'Present' : 'Absent'),
                col: moment(re.startTime).format("dddd").toLowerCase(),
                row: unit ? unit.id : 0,
                present: re.present,
                date: re.startTime.toString()
            }
            return event;
        });
    }

    private mapToColumns() {
        const { startDate, endDate } = this.state;
        const range = moment().range(startDate, endDate);
        let dates = Array.from(range.by("day"));
        return dates.map((date: Moment.Moment) => {
            return {
                id: date.format("dddd").toLowerCase(),
                name: date.format("ddd, DD MMM YYYY")
            }
        });
    }


    private ensureDataFetched() {
        var recordSearch: RecordSearch = {
            attendeeId: this.state.attendeeId,
            startTime: this.state.startDate,
            endTime: this.state.endDate
        }
        this.props.requestRecords(recordSearch);
    }

}

const mapDispatchToProps = (dispatch: any) => {
    return bindActionCreators({ ...recordActionCreators, ...unitActionCreators }, dispatch);
}

const mapStateToProps = (state: ApplicationState) => {
    return { ...state.record, ...state.units };
}

export default connect(mapStateToProps, mapDispatchToProps)(Record);
