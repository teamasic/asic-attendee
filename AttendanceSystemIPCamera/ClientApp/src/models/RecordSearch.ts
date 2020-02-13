import Attendee from "./Attendee";

export default interface RecordSearch {
    attendeeId: number;
    startTime: Date;
    endTime: Date;
}