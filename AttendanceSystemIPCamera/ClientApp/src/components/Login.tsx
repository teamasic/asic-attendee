import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import { attendeeActionCreators } from '../store/attendee/attendeeActionCreators';
import { ApplicationState } from '../store';
import { AttendeeState } from '../store/attendee/attendeeState';

import { Form, Icon, Input, Button, Checkbox, Spin, Tabs } from 'antd';
import Webcam from 'react-webcam';
import AttendeeLogin from '../models/AttendeeLogin';
import { UncontrolledDropdown } from 'reactstrap';
import { loginMethod } from '../constant';
import { login } from '../services/attendee';



// At runtime, Redux will merge together...
type AttendeeProps =
    AttendeeState // ... state we've requested from the Redux store
    & typeof attendeeActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

interface LoginComponentState {
    // attendeeCode: string = "";
    // faceData: string = "";
    // loginMethod: string = "1";
    // webcamEnabled: boolean = false;
    attendeeCode: string,
    faceData: string,
    loginMethod: string,
    webcamEnabled: boolean
}

const redirectLocation = '/record';
const { TabPane } = Tabs;

class Login extends React.PureComponent<AttendeeProps, LoginComponentState> {

    videoConstraints = {
        width: 1280,
        height: 720,
        facingMode: "user"
    };

    constructor(props: AttendeeProps) {
        super(props);
        this.state = {
            attendeeCode: "",
            faceData: "",
            loginMethod: "3",
            webcamEnabled: false
        };
        console.log(this.props);
        console.log(this.state);
    }

    redirect = () => {
        window.location.replace(redirectLocation);
    }

    handleSubmit = (e: any) => {
        e.preventDefault();
        console.log(this.state.loginMethod);
        let attendeeLogin = null;

        switch (this.state.loginMethod) {
            case loginMethod.lOGIN_BY_ATTENDEE_CODE: // login by username password
                attendeeLogin = {
                    loginMethod: this.state.loginMethod,
                    attendeeCode: this.state.attendeeCode,
                    faceData: ""
                };
                break;

            case loginMethod.LOGIN_BY_FACE: // login by face
                const imageSrc = (this.refs.webcam as Webcam).getScreenshot();
                console.log(imageSrc);
                attendeeLogin = {
                    loginMethod: this.state.loginMethod,
                    faceData: imageSrc ? imageSrc : "",
                    attendeeCode: ""
                };
                break;
        }

        if (attendeeLogin != null) {
            this.props.requestLogin(attendeeLogin, this.redirect);
        }
    }

    handleChangeTab = (key: string) => {
        this.setState({ loginMethod: key, webcamEnabled: !this.state.webcamEnabled });
    }

    public render() {
        let { attendeeCode } = this.state;
        return (
            <div className="container">
                <div className="content">
                    <Tabs defaultActiveKey={this.state.loginMethod} onChange={this.handleChangeTab}>
                        <TabPane tab="Login by username and password" key={loginMethod.lOGIN_BY_ATTENDEE_CODE}>
                            <Form onSubmit={(e) => this.handleSubmit(e)} className="login-form">
                                <Form.Item>
                                    <Input
                                        prefix={<Icon type="user" style={{ color: 'rgba(0,0,0,.25)' }} />}
                                        placeholder="Attendee Code"
                                        onChange={(e) => this.setState({ attendeeCode: e.target.value })}
                                        defaultValue={attendeeCode}
                                    />
                                </Form.Item>
                                <Form.Item>
                                    <Button type="primary" htmlType="submit" className="login-form-button">Log in</Button>
                                </Form.Item>
                            </Form>

                        </TabPane>

                        <TabPane tab="Login by face" key={loginMethod.LOGIN_BY_FACE}>

                            <Form onSubmit={(e) => this.handleSubmit(e)} className="login-form">
                                {this.state.webcamEnabled ? <React.Fragment>
                                    <Webcam
                                        audio={false}
                                        height={350}
                                        ref="webcam"
                                        screenshotFormat="image/jpeg"
                                        width={350}
                                        videoConstraints={this.videoConstraints}
                                    />
                                </React.Fragment>
                                    : ""}
                                <Form.Item>
                                    <Button type="primary" htmlType="submit" className="login-form-button">Log in</Button>
                                </Form.Item>
                            </Form>
                        </TabPane>

                    </Tabs>
                    {
                        (this.props.isLoading) ? <Spin /> : ""
                    }
                </div>
            </div>
        );
    }


}

const matchDispatchToProps = (dispatch: any) => {
    return bindActionCreators(attendeeActionCreators, dispatch);
}

export default connect((state: ApplicationState) => state.attendee, matchDispatchToProps)(Login);

