import ApiResponse from "../../models/ApiResponse";
import { AppThunkAction } from "..";
import RecordSearch from "../../models/RecordSearch";
import Record, { RecordViewModel } from "../../models/Record";
import { getRecords } from "../../services/record";

export const ACTIONS = {
    START_REQUEST_RECORDS: 'START_REQUEST_RECORDS',
    STOP_REQUEST_RECORDS_WITH_ERRORS: 'STOP_REQUEST_RECORDS_WITH_ERRORS',
    RECEIVE_RECORDS_DATA: 'RECEIVE_RECORDS_DATA'
}

function startRequestRecords(recordSearch: RecordSearch) {
    return {
        type: ACTIONS.START_REQUEST_RECORDS,
        recordSearch
    };
}

function stopRequestRecordsWithError(errors: any[]) {
    return {
        type: ACTIONS.STOP_REQUEST_RECORDS_WITH_ERRORS,
        errors
    };
}

function receiveRecordsData(records: any[]) {
    let data = records.map(record => (
        {
            ...record,
            startTime: new Date(record.startTime)
        }
    ));
    return {
        type: ACTIONS.RECEIVE_RECORDS_DATA,
        records: data
    };
}

const requestRecords = (recordSearch: RecordSearch): AppThunkAction => async (dispatch, getState) => {
    dispatch(startRequestRecords(recordSearch));

    const apiResponse: ApiResponse = await getRecords(recordSearch);
    if (apiResponse.success) {
        console.log(apiResponse.data)
        dispatch(receiveRecordsData(apiResponse.data));
    } else {
        dispatch(stopRequestRecordsWithError(apiResponse.errors));
    }
}

export const recordActionCreators = {
    requestRecords: requestRecords
};