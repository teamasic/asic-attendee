import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';
import { recordActionCreators } from '../store/record/recordActionCreators';
import { RecordState } from '../store/record/recordState';
import { Calendar, momentLocalizer, View, NavigateAction, Event } from 'react-big-calendar'
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
    showDate: Date
}

const localizer = momentLocalizer(moment);

class Record extends React.PureComponent<RecordProps, RecordComponentState> {

    allowComponentDidUpdateRun = false
    today = new Date();
    min = new Date(0, 0, 0, 7, 0, 0);
    max = new Date(0, 0, 0, 21, 0, 0);

    constructor(props: RecordProps) {
        super(props);
        let attendeeStr = localStorage.getItem(constants.AUTH_IN_LOCAL_STORAGE);
        let attendee: Attendee = JSON.parse(attendeeStr ? attendeeStr : "");

        this.state = {
            attendeeId: attendee.id,
            startRange: moment(this.today).startOf('week').toDate(),
            endRange: moment(this.today).endOf('week').toDate(),
            showDate: this.today
        };
        console.log(this.props);
    }

    componentDidMount() {
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
                <Calendar
                    events={this.mapDataToRecordViewModel()}
                    views={['week']}
                    // step={60}
                    showMultiDayTimes
                    localizer={localizer}
                    onNavigate={this.onNavigate}
                    defaultView='week'
                    date={this.state.showDate}
                    eventPropGetter={this.eventStyleGetter}
                    min={this.min}
                    max={this.max}
                    // timeslots={1}
                />
        );
    }

    eventStyleGetter = (event: Event) => {
        console.log(event);
        var backgroundColor = "green";
        if (event.resource.present) {
            backgroundColor = "red"
        }
        var style = {
            backgroundColor: backgroundColor,
        };
        return {
            style: style
        };
    }


    onNavigate = (date: Date, view: View) => {
        this.allowComponentDidUpdateRun = true;
        console.log(date);
        switch (view) {
            case 'week':
                const start = moment(date).startOf('week').toDate();
                const end = moment(date).endOf('week').toDate();
                console.log(start)
                console.log(end)
                this.setState({ startRange: start, endRange: end, showDate: date });
                break;
        }
    }

    private mapDataToRecordViewModel() {
        if (this.props.recordData) {
            let data = this.props.recordData.map(rd => ({
                id: rd.id,
                title: rd.groupCode,
                start: rd.startTime,
                end: moment(rd.startTime).add(rd.duration, 'm').toDate(),
                allDay: false,
                resource: {
                    present: rd.present
                }
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

const mapDispatchToProps = (dispatch: any) => {
    return bindActionCreators(recordActionCreators, dispatch);
}

export default connect((state: ApplicationState) => state.record, mapDispatchToProps)(Record);
