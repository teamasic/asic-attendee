import Attendee from "../../models/Attendee";

export interface AttendeeState {
    isLoading: boolean;
    successfullyLoaded: boolean;
    attendee: Attendee;
    isLogin: boolean;
    errors: any[];
}