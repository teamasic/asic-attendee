export default interface Record {
    id: number;
    groupCode: string;
    startTime: Date;
    duration: number;
    present: boolean
}

export interface RecordViewModel{
    id: number;
    title: string,
    start: Date,
    end: Date,
    allDay: boolean
}