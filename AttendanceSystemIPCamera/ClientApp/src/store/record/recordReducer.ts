import { Reducer, Action, AnyAction } from "redux";
import { RecordState } from "./recordState";
import { ACTIONS } from './recordActionCreators';

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: RecordState = {
    isLoading: false,
    successfullyLoaded: false,
    recordSearch: {
        attendeeId: 0,
        startTime: new Date(),
        endTime: new Date()
    },
    recordData: [{
        id: 0,
        name: "",
        groupCode: "",
        groupName: "",
        startTime: new Date(),
        endTime: new Date(),
        present: false
    }],
    errorsInRecordState:[]
};

const reducers: Reducer<RecordState> = (state: RecordState | undefined, incomingAction: AnyAction): RecordState => {
    if (state === undefined) {
        return unloadedState;
    }
    const action = incomingAction;
    switch (action.type) {
        case ACTIONS.START_REQUEST_RECORDS:
            return {
                ...state,
                isLoading: true,
                successfullyLoaded: false,
                recordSearch: action.recordSearch
            };
        case ACTIONS.STOP_REQUEST_RECORDS_WITH_ERRORS:
            return {
                ...state,
                isLoading: false,
                successfullyLoaded: false,
                errorsInRecordState: action.errors
            };
        case ACTIONS.RECEIVE_RECORDS_DATA:
            return {
                ...state,
                isLoading: false,
                successfullyLoaded: true,
                recordData: action.records
            };
    }

    return state;
};

export default reducers;