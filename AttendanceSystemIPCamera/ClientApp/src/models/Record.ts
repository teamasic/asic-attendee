export default interface Record {
    id: number;
    name: string;
    groupCode: string;
    startTime: Date;
    endTime: Date;
    present: boolean
}

export interface RecordViewModel{
    id: number;
    title: string,
    start: Date,
    end: Date,
    allDay: boolean
}