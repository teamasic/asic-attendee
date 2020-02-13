import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';
import { recordActionCreators } from '../store/record/recordActionCreators';
import { RecordState } from '../store/record/recordState';
import { Calendar, momentLocalizer, View } from 'react-big-calendar'
import moment from 'moment';
import { constants } from '../constant';
import Attendee from '../models/Attendee';
import RecordSearch from '../models/RecordSearch';
import { Spin } from 'antd';

import "react-big-calendar/lib/css/react-big-calendar.css";

// At runtime, Redux will merge together...
type RecordProps =
    RecordState // ... state we've requested from the Redux store
    & typeof recordActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

interface RecordComponentState {
    attendeeId: number;
    endRange: Date;
    startRange: Date;

}

const localizer = momentLocalizer(moment);
const today = new Date(2020, 0, 19);

class Record extends React.PureComponent<RecordProps, RecordComponentState> {

    constructor(props: RecordProps) {
        super(props);
        let attendeeStr = localStorage.getItem(constants.AUTH_IN_LOCAL_STORAGE);
        let attendee: Attendee = JSON.parse(attendeeStr ? attendeeStr : "");

        this.state = {
            attendeeId: attendee.id,
            startRange: moment(today).startOf('week').toDate(),
            endRange: moment(today).endOf('week').toDate()
        };
    }

    componentDidMount() {
        this.ensureDataFetched();
    }

    componentDidUpdate() {
        this.ensureDataFetched();
    }

    public render() {
        return (
            (this.props.isLoading) ? <Spin /> :
                <Calendar
                    events={this.mapDataToRecordViewModel()}
                    views={['week']}
                    step={60}
                    showMultiDayTimes
                    localizer={localizer}
                    onNavigate={this.onNavigate}
                    defaultView='week'
                    date={today}
                />
        );
    }

    onNavigate = (date: Date, view: View) => {
        switch (view) {
            case 'week':
                const start = moment(date).startOf('week').toDate();
                const end = moment(date).endOf('week').toDate();
                console.log(start)
                console.log(end)
                this.setState({ startRange: start, endRange: end });
                break;
        }
    }

    private mapDataToRecordViewModel() {
        console.log(this.props)
        console.log(this.props.recordData)
        console.log(this.props.recordSearch)
        if (this.props.recordData) {
            let data = this.props.recordData.map(rd => ({
                id: rd.id,
                title: rd.groupCode,
                start: rd.startTime,
                end: moment(rd.startTime).add(rd.duration, 'm').toDate(),
                allDay: false,
            }));
            return data;
        } else {
            return [];
        }
    }

    private ensureDataFetched() {
        var recordSearch: RecordSearch = {
            attendeeId: this.state.attendeeId,
            startTime: this.state.startRange,
            endTime: this.state.endRange
        }
        this.props.requestRecords(recordSearch);
    }

}

const mapStateToProps = (state: ApplicationState) => {
    console.log(state);
    return { state: state.record };
}

const mapDispatchToProps = (dispatch: any) => {
    return bindActionCreators(recordActionCreators, dispatch);
}

export default connect(mapStateToProps, mapDispatchToProps)(Record);