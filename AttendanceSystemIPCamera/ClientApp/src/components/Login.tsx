import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import { attendeeActionCreators } from '../store/attendee/attendeeActionCreators';
import { ApplicationState } from '../store';
import { AttendeeState } from '../store/attendee/attendeeState';

import { constants } from '../constant';
import { Form, Icon, Input, Button, Checkbox, Spin } from 'antd';


// At runtime, Redux will merge together...
type AttendeeProps =
    AttendeeState // ... state we've requested from the Redux store
    & typeof attendeeActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

interface LoginComponentState {
    attendeeCode: string
}

const redirectLocation = '/record';

class Login extends React.PureComponent<AttendeeProps, LoginComponentState> {
    constructor(props: AttendeeProps) {
        super(props);
        this.state = {
            attendeeCode: ""
        };
        console.log(this.props);
        console.log(this.state);
    }

    redirect = () => {
        window.location.replace(redirectLocation);
        // let { history } = this.props;
        // history.push(redirectLocation);
    }

    private handleSubmit(e: any) {
        e.preventDefault();
        let attendeeLogin = {
            attendeeCode: this.state.attendeeCode
        };
        this.props.requestLogin(attendeeLogin, this.redirect);
    }

    public render() {
        let { attendeeCode } = this.state;
        return (
            <div>
                <Form onSubmit={(e) => this.handleSubmit(e)} className="login-form">
                    <Form.Item>
                        <Input
                            prefix={<Icon type="user" style={{ color: 'rgba(0,0,0,.25)' }} />}
                            placeholder="Attendee Code"
                            onChange={(e) => this.setState({ attendeeCode: e.target.value })}
                            defaultValue={attendeeCode}
                        />
                    </Form.Item>
                    {/* <Form.Item>
                        <Input
                            prefix={<Icon type="lock" style={{ color: 'rgba(0,0,0,.25)' }} />}
                            type="password"
                            placeholder="Password"
                        />
                </Form.Item> */}
                    <Form.Item>
                        <Button type="primary" htmlType="submit" className="login-form-button">
                            Log in
              </Button>
                    </Form.Item>
                </Form>
                {
                    this.props.isLoading ? <Spin /> : <div></div>
                }
            </div>
        );
    }

}

const matchDispatchToProps = (dispatch: any) => {
    return bindActionCreators(attendeeActionCreators, dispatch);
}

export default connect((state: ApplicationState) => state.attendee, matchDispatchToProps)(Login);

