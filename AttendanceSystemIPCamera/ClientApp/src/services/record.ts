import ApiResponse from "../models/ApiResponse";
import { constants } from "../constant";
import axios from "axios";
import RecordSearch from "../models/RecordSearch";

const baseRoute  = constants.BASE_ROUTE + "record";
const apify = (path: string) => `${baseRoute}/${path}`;

const transformChangeRequestRecords = (data: any) => {
    data.data = data.data.map((r: any) => r.changeRequest != null ? ({
        ...r,
        changeRequest: {
            recordId: r.id,
            groupName: r.groupName,
            groupCode: r.groupCode,
            sessionTime: r.startTime,
            comment: r.changeRequest.comment,
            status: r.changeRequest.status
        }
    }) : r);
};

export const getRecords = async (recordSearch: RecordSearch): Promise<ApiResponse> => {
    const response = await axios.get(baseRoute, {
        params: recordSearch
    });
    const data = await response.data;
    transformChangeRequestRecords(data);
    return data;
};

export const refreshRecords = async (recordSearch: RecordSearch): Promise<ApiResponse> => {
    const response = await axios.get(apify("refresh"), {
        params: recordSearch
    });
    const data = await response.data;
    transformChangeRequestRecords(data);
    return data;
};

