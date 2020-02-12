import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';
import { Typography } from 'antd';
import { Input } from 'antd';
import { recordActionCreators } from '../store/record/recordActionCreators';
import { RecordState } from '../store/record/recordState';
import {Calendar, momentLocalizer } from 'react-big-calendar'
import moment from 'moment';
import { constants, attendanceData } from '../constant';

import "react-big-calendar/lib/css/react-big-calendar.css";


// At runtime, Redux will merge together...
type RecordProps =
    RecordState // ... state we've requested from the Redux store
    & typeof recordActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

const localizer = momentLocalizer(moment);
const events = attendanceData;

class Record extends React.PureComponent<RecordProps> {
    public render() {
        return (
            <Calendar
                events={events}
                views={['month', 'week', 'day']}
                step={60}
                showMultiDayTimes
                defaultDate={new Date()}
                localizer={localizer}
            />
        );
    }

}

export default connect(
    (state: ApplicationState) => state.record, // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(recordActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(Record);
