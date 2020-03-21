import ApiResponse from "../../models/ApiResponse";
import { ThunkDispatch } from "redux-thunk";
import { AppThunkAction } from "..";
import { AnyAction } from "redux";
import Attendee from "../../models/Attendee";
import AttendeeLogin from "../../models/AttendeeLogin";
import { login, loginWithFirebase } from "../../services/attendee";
import { constants } from "../../constant";
import UserLogin from "../../models/UserLogin";

export const ACTIONS = {
    START_REQUEST_LOGIN:"START_REQUEST_LOGIN",
    STOP_REQUEST_LOGIN_WITH_ERRORS:"STOP_REQUEST_LOGIN_WITH_ERRORS",
    RECEIVE_SUCCESS_LOGIN:"RECEIVE_SUCCESS_LOGIN"

}

function startRequestLogin() {
    return {
        type: ACTIONS.START_REQUEST_LOGIN
    };
}

function stopRequestLoginWithError(errors: any[]) {
    return {
        type: ACTIONS.STOP_REQUEST_LOGIN_WITH_ERRORS,
        errors
    };
}

function receiveSuccessLogin(attendee:Attendee) {
    return {
        type: ACTIONS.RECEIVE_SUCCESS_LOGIN,
        attendee
    };
}

const requestLogin = (attendeeLogin: AttendeeLogin, redirect: Function): AppThunkAction => async (dispatch, getState) => {
    dispatch(startRequestLogin());

    const apiResponse: ApiResponse = await login(attendeeLogin);
    if (apiResponse.success) {
        console.log(apiResponse.data);
        localStorage.setItem(constants.AUTH_IN_LOCAL_STORAGE, JSON.stringify(apiResponse.data));
        dispatch(receiveSuccessLogin(apiResponse.data));
        redirect();
    } else {
        dispatch(stopRequestLoginWithError(apiResponse.errors));
    }
}


const requestLoginWithFirebase = (userLogin: UserLogin, redirect: Function): AppThunkAction => async (dispatch, getState) => {
    dispatch(startRequestLogin());

    const apiResponse: ApiResponse = await loginWithFirebase(userLogin);
    if (apiResponse.success) {
        console.log(apiResponse.data);
        localStorage.setItem(constants.AUTH_IN_LOCAL_STORAGE, JSON.stringify(apiResponse.data));
        dispatch(receiveSuccessLogin(apiResponse.data));
        redirect();
    } else {
        dispatch(stopRequestLoginWithError(apiResponse.errors));
    }
}

export const attendeeActionCreators = {
    requestLogin,
    requestLoginWithFirebase
};