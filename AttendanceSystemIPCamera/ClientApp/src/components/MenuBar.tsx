import * as React from 'react';
import { ApplicationState } from '../store';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import NavMenu from './NavMenu';
import '../styles/MenuBar.css';
import { Layout, Menu, Breadcrumb, Icon, Dropdown, Badge, Row, Col, Spin, Avatar, Button } from 'antd';
import classNames from 'classnames';
import { Link, withRouter } from 'react-router-dom';
import { RouteComponentProps } from 'react-router';
import { constants } from '../constant';
import * as firebase from '../firebase';
import { AttendeeState } from '../store/attendee/attendeeState';
import { attendeeActionCreators } from '../store/attendee/attendeeActionCreators';
const { Header, Sider, Content, Footer } = Layout;

// At runtime, Redux will merge together...
type Props =
	AttendeeState &
	typeof attendeeActionCreators &
	RouteComponentProps<{}>; // ... plus incoming routing parameters


class MenuBar extends React.Component<Props> {
	render() {
		return <Content className="menu-bar row">
			<div className="fullname">{this.props.attendee.name}</div>
			<Avatar className="avatar" src={this.props.attendee.image} />
			</Content>;
	}
}

export default withRouter(connect(
	(state: ApplicationState) => ({
		...state.attendee,
	}), // Selects which state properties are merged into the component's props
	dispatch => bindActionCreators({
	}, dispatch) // Selects which action creators are merged into the component's props
)(MenuBar as any));