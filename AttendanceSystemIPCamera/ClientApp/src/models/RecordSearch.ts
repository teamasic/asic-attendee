import Attendee from "./Attendee";

export default interface RecordSearch {
    attendeeCode: string;
    startTime: Date;
    endTime: Date;
}