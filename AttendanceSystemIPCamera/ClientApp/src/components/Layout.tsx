import * as React from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';
import { Layout, Menu, Breadcrumb, Icon, Divider, Row, Col } from 'antd';
import '../styles/Layout.css';
import { constants } from '../constant';
import * as firebase from '../firebase';

import { AttendeeState } from '../store/attendee/attendeeState';
import { attendeeActionCreators } from '../store/attendee/attendeeActionCreators';
import { bindActionCreators } from 'redux';
import { withRouter } from 'react-router';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import MenuBar from './MenuBar';
const { Header, Sider, Content, Footer } = Layout;

type LayoutProps =
	AttendeeState &
	typeof attendeeActionCreators;

class PageLayout extends React.Component<
	LayoutProps,
	{
		collapsed: boolean;
	}
	> {
	state = {
		collapsed: false
	};


	constructor(props: any) {
		super(props);
	}

	onCollapse = (collapsed: boolean) => {
		this.setState({ collapsed });
	};

	render() {
		return (<>{this.props.isLogin ? this.renderLayout() : this.renderEmty()}</>);
	}
	private renderLayout() {
		return (
			<Layout className="layout">
				<Sider
					className="sider"
					collapsible
					collapsed={this.state.collapsed}
					onCollapse={this.onCollapse}
				>
					<div className="logo">ASIC</div>
					<Menu theme="dark" defaultSelectedKeys={['1']} mode="inline">
						<Menu.Item key="1">
							<Icon type="hdd" />
							<span>Your groups</span>
						</Menu.Item>
						<Menu.Item key="3" onClick={this.logout}>
							<Icon type="logout" />
							<span>Logout</span>
						</Menu.Item>
					</Menu>
				</Sider>
				<Layout>
					<MenuBar />
					<Content className="content">
						{this.props.children}
					</Content>
				</Layout>
			</Layout >

		);
	}
	private renderEmty() {
		return (
			<Layout className="layout">
				<Row type='flex' align='middle' justify='space-around' >
					<Col span={8} >
						{this.props.children}
					</Col>
				</Row>
			</Layout>
		);
	}

	private logout() {
		const authData = localStorage.getItem(constants.AUTH_IN_LOCAL_STORAGE);
		if (authData != null) {
			firebase.auth.doSignOut();
			localStorage.removeItem(constants.AUTH_IN_LOCAL_STORAGE);
			window.location.href = "/";
		}
	}
}

export default withRouter(connect(
	(state: ApplicationState) => ({
		...state.attendee
	}), // Selects which state properties are merged into the component's props
	dispatch => bindActionCreators({
		...attendeeActionCreators
	}, dispatch) // Selects which action creators are merged into the component's props
)(PageLayout as any));
