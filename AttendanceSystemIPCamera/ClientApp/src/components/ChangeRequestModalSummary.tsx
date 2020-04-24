import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { bindActionCreators } from 'redux';
import Group from '../models/Group';
import Record from '../models/Record';
import { ApplicationState } from '../store';
import { changeRequestActionCreators } from '../store/changeRequest/actionCreators';
import { ChangeRequestState } from '../store/changeRequest/state';
import { Card, Button, Dropdown, Icon, Menu, Row, Col, Modal, Form, Input, message } from 'antd';
import { Typography } from 'antd';
import classNames from 'classnames';
import ChangeRequest, { ChangeRequestStatus } from '../models/ChangeRequest';
import { Link } from 'react-router-dom';
import { formatDateString } from '../utils';

const { Title } = Typography;
const { confirm, success, error } = Modal;

interface ChangeRequestModalState {
    comment: string;
}

interface Props {
    changeRequest: ChangeRequest;
    visible: boolean;
    hideModal: () => void;
}

class ChangeRequestModalSummary extends React.PureComponent<Props, ChangeRequestModalState> {
    state = {
        comment: ''
    };

    public render() {
        const cr = this.props.changeRequest;
        if (cr == null) return <></>;
        let status;
        switch (cr.status) {
            case ChangeRequestStatus.APPROVED:
                status = <>
                    <Icon type="check-circle" />
                    <span className="status">Approved</span>
                </>;
                break;
            case ChangeRequestStatus.REJECTED:
                status = <>
                    <Icon type="close-circle" />
                    <span className="status">Rejected</span>
                </>;
                break;
            case ChangeRequestStatus.UNRESOLVED:
                status = <>
                    <Icon type="question-circle" />
                    <span className="status">Unresolved</span>
                </>;
                break;
            case ChangeRequestStatus.EXPIRED:
                status = <>
                    <Icon type="stop" />
                    <span className="status">Expired</span>
                </>;
                break;
        }
        let sessionTime;
        try {
            sessionTime = formatDateString(cr.sessionTime);
        } catch (e) {
            sessionTime = "";
        }
        return (
            <Modal
                title="Change request details"
                className="change-request-modal"
                visible={this.props.visible}
                onOk={() => { }}
                onCancel={() => this.props.hideModal()}
                footer={[
                    <Button key="cancel"
                        onClick={() => this.props.hideModal()}>
                        Close
                    </Button>
                ]}
            >
                <Row className="info-wrapper">
                    <Col span={4}><span className="label shifted-down">Group:</span></Col>
                    <Col span={20} className="info">{cr.groupCode + " - " + cr.groupName}</Col>
                </Row>
                <Row className="info-wrapper">
                    <Col span={4}><span className="label shifted-down">Date:</span></Col>
                    <Col span={20} className="info">{sessionTime}</Col>
                </Row>
                <Row className="info-wrapper">
                    <Col span={4}><span className="label">Status:</span></Col>
                    <Col className="">{status}</Col>
                </Row>
                <Row className="info-wrapper">
                    <Col span={4}><span className="label">Comment:</span></Col>
                    <Col span={20} className="comment-wrapper">{cr.comment}</Col>
                </Row>
            </Modal>
        );
    }
}

export default ChangeRequestModalSummary;