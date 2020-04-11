import RecordSearch from "../../models/RecordSearch";
import Record, { RecordViewModel } from "../../models/Record";

export interface RecordState {
    isLoading: boolean;
    successfullyLoaded: boolean;
    recordSearch: RecordSearch;
    recordData: Record[];
    errorsInRecordState: any[]
}