import ApiResponse from "../models/ApiResponse";
import { constants } from "../constant";
import axios from "axios";
import RecordSearch from "../models/RecordSearch";

const baseRoute  = constants.BASE_ROUTE + "record";
const apify = (path: string) => `${baseRoute}/${path}`;

export const getRecords = async (recordSearch: RecordSearch): Promise<ApiResponse> => {
    const response = await axios.get(baseRoute, {
        params: recordSearch
    });
    return await response.data;
};

export const refreshRecords = async (recordSearch: RecordSearch): Promise<ApiResponse> => {
    const response = await axios.get(apify("refresh"), {
        params: recordSearch
    });
    return await response.data;
};

