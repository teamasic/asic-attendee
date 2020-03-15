import ChangeRequest from "./ChangeRequest";

export default interface Record {
    id: number;
    name: string;
    groupCode: string;
    groupName: string;
    startTime: Date;
    endTime: Date;
    present: boolean
    changeRequest?: ChangeRequest;
}

export interface RecordViewModel{
    id: number;
    title: string,
    start: Date,
    end: Date,
    allDay: boolean
}