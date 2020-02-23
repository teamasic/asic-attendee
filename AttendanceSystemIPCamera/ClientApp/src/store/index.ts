import { GroupsState } from './group/state';
import GroupsReducer from './group/reducers';
import AttendeeReducer from './attendee/attendeeReducer';
import RecordReducer from './record/recordReducer';
import UnitsReducer from './unit/reducers';
import { AnyAction } from 'redux';
import { AttendeeState } from './attendee/attendeeState';
import { RecordState } from './record/recordState';
import { UnitsState } from './unit/state';

// The top-level state object
export interface ApplicationState {
    groups: GroupsState | undefined;
    attendee: AttendeeState | undefined;
    record: RecordState | undefined;
    units: UnitsState | undefined;
}

// Whenever an action is dispatched, Redux will update each top-level application state property using
// the reducer with the matching name. It's important that the names match exactly, and that the reducer
// acts on the corresponding ApplicationState property type.
export const reducers = {
    groups: GroupsReducer,
    attendee: AttendeeReducer,
    record: RecordReducer,
    units: UnitsReducer

};

// This type can be used as a hint on action creators so that its 'dispatch' and 'getState' params are
// correctly typed to match your store.
export interface AppThunkAction {
    (dispatch: (action: AnyAction) => void, getState: () => ApplicationState): void;
}