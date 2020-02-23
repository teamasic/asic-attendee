import * as React from 'react';
import { Route, Redirect } from 'react-router';
import Layout from './components/Layout';
import Login from './components/Login';

import './App.css';
import Dashboard from './components/Dashboard';
import Session from './components/Session';
import { constants } from './constant';
import Record from './components/Record';
import TableRecord from './components/TableRecord';

export default () => {
    const authData = localStorage.getItem(constants.AUTH_IN_LOCAL_STORAGE);
    if (authData) {
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
};



