import ApiResponse from "../models/ApiResponse";
import { constants } from "../constant";
import axios from "axios";
import AttendeeLogin from "../models/AttendeeLogin";
import UserLogin from "../models/UserLogin";

const baseRoute  = constants.BASE_ROUTE + "attendee";
const apify = (path: string) => `${baseRoute}/${path}`;

export const login = async (attendeeLogin: AttendeeLogin): Promise<ApiResponse> => {
    const response = await axios.post(apify('login'), attendeeLogin);
    return await response.data;
};

export const loginWithFirebase = async (userLogin: UserLogin): Promise<ApiResponse> => {
    const response = await axios.post(apify('login/firebase'), userLogin);
    return await response.data;
};