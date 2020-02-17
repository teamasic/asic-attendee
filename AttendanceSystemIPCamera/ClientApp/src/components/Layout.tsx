import * as React from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';
import { Layout, Menu, Breadcrumb, Icon, Divider, Row, Col } from 'antd';
import '../styles/Layout.css';
import { constants } from '../constant';

const { Header, Sider, Content, Footer } = Layout;

class PageLayout extends React.Component<
	any,
	{
		collapsed: boolean;
	}
	> {
	state = {
		collapsed: false
	};

	onCollapse = (collapsed: boolean) => {
		this.setState({ collapsed });
	};

	render() {
		const authData = localStorage.getItem(constants.AUTH_IN_LOCAL_STORAGE);
		console.log(authData);
		return (<div>{authData ? this.renderLayout() : this.renderEmty()}</div>);
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
						<Menu.Item key="2">
							<Icon type="sync" />
							<span>Sync</span>
						</Menu.Item>
					</Menu>
				</Sider>
				<Layout>
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
}

export default PageLayout;
