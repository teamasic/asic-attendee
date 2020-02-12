import { Reducer, Action, AnyAction } from "redux";
import { RecordState } from "./recordState";


// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: RecordState = {
    isLoading: false,
    successfullyLoaded: false,
};

const reducers: Reducer<RecordState> = (state: RecordState | undefined, incomingAction: AnyAction): RecordState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction;
    switch (action.type) {
        
    }

    return state;
};

export default reducers;