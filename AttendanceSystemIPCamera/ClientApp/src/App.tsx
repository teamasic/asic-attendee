import * as React from 'react';
import { Route, Redirect } from 'react-router';
import Layout from './components/Layout';
import Login from './components/Login';

import './App.css';
import { constants } from './constant';
import Record from './components/Record';
import { connect } from 'react-redux';
import { ApplicationState } from './store';
import { AttendeeState } from './store/attendee/attendeeState';
import { attendeeActionCreators } from './store/attendee/attendeeActionCreators';
import { bindActionCreators } from 'redux';

type AppProps =
    AttendeeState &
    typeof attendeeActionCreators;

class AppComponent extends React.Component<AppProps> {

    constructor(props: any) {
        super(props);
    }
    componentDidMount() {
        if (!this.props.isLogin) {
            this.props.checkUserInfo();
        }
    }

    public render() {
        if (!this.props.successfullyLoaded) {
            return (
                <Layout>
                </Layout>
            );
        }
        if (this.props.isLogin) {
            console.log(this.props.attendee);
            return (
                <Layout>
                    <Route exact path='/'>
                        <Redirect exact to='/record' />
                    </Route>
                    <Route exact path="/record" component={Record} />
                </Layout>
            );
        } else {
            return (
                <Layout>
                    <Route exact path="/" component={Login} />
                </Layout>
            );
        }
    }
}
const matchDispatchToProps = (dispatch: any) => {
    return bindActionCreators(attendeeActionCreators, dispatch);
}

export default connect((state: ApplicationState) => state.attendee, matchDispatchToProps)(AppComponent);



