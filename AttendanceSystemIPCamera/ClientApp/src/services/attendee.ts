import ApiResponse from "../models/ApiResponse";
import { constants } from "../constant";
import axios from "axios";
import AttendeeLogin from "../models/AttendeeLogin";

const baseRoute  = constants.BASE_ROUTE + "attendee";
const apify = (path: string) => `${baseRoute}/${path}`;

export const login = async (attendeeLogin: AttendeeLogin): Promise<ApiResponse> => {
    const response = await axios.post(apify('login'), attendeeLogin);
    return await response.data;
};