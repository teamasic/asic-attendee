import { Reducer, Action, AnyAction } from "redux";
import { AttendeeState } from "./attendeeState";
import { ACTIONS } from "./attendeeActionCreators";

// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: AttendeeState = {
    isLoading: false,
    successfullyLoaded: false,
    attendee: {
        id: 0,
        code: "",
        name: ""
    }
};

const reducers: Reducer<AttendeeState> = (state: AttendeeState | undefined, incomingAction: AnyAction): AttendeeState => {
    if (state === undefined) {
        return unloadedState;
    }
    const action = incomingAction;
    switch (action.type) {
        case ACTIONS.START_REQUEST_LOGIN:
            return {
                ... state,
                isLoading: true,
                successfullyLoaded: false,
            };
        case ACTIONS.STOP_REQUEST_LOGIN_WITH_ERRORS:
            return {
                ... state,
                isLoading: false,
                successfullyLoaded: false
            };
        case ACTIONS.RECEIVE_SUCCESS_LOGIN:
            return {
                ...state,
                attendee: action.attendee,
                isLoading: false,
                successfullyLoaded: true
            };
    }

    return state;
};

export default reducers;