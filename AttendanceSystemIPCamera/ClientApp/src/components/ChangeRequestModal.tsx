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

const { Title } = Typography;
const { confirm, success, error } = Modal;

interface ChangeRequestModalState {
    comment: string;
}

interface Props {
    record?: Record;
    visible: boolean;
    hideModal: () => void;
}

// At runtime, Redux will merge together...
type ChangeRequestProps =
    Props &
    ChangeRequestState // ... state we've requested from the Redux store
    & typeof changeRequestActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

class ChangeRequestModal extends React.PureComponent<ChangeRequestProps, ChangeRequestModalState> {
    state = {
        comment: ''
    };

    private updateComment(val: string) {
        this.setState({
            comment: val
        });
    }

    private submit() {
        if (this.props.record) {
            this.props.createChangeRequest({
                recordId: this.props.record!.id,
                present: !this.props.record!.present, // the desired state
                comment: this.state.comment,
            }, this.props.record, this.onSubmitSuccess, this.onSubmitError)
        }
    }

    private onSubmitSuccess = () => {
        this.props.hideModal();
        success({
            title: 'Successfully created change request'
        });
    };

    private onSubmitError = () => {
        error({
            title: 'Error in processing change request',
            content: 'Please try again.'
        });
    };

    public render() {
        const record = this.props.record;
        return (
            <Modal
                className="change-request-modal"
                title="Request change to this record"
                visible={this.props.visible}
                onCancel={() => this.props.hideModal()}
                footer={[
                    <Button key="back" onClick={() => this.props.hideModal()}>
                        Cancel
                    </Button>,
                    <Button key="submit" type="primary" onClick={() => this.submit()}>
                        Submit
                    </Button>,
                ]}
            >
                {
                    record != null &&
                    <Form name="basic">
                        <Row className="item">
                            <Col span={4}><span className="label">Group:</span></Col>
                            <Col>{record.groupCode + " - " + record.groupName}</Col>
                        </Row>
                        <Row className="item">
                            <Col span={4}><span className="label">Date:</span></Col>
                            <Col>{record.startTime.toLocaleString()}</Col>
                        </Row>
                        <Row className="item">
                            <Col span={4}><span className="label">Status:</span></Col>
                            <Col>{record.present ? "Present" : "Absent"}</Col>
                        </Row>
                        <Form.Item
                            label="Additional comment"
                            extra="Maximum 200 characters">
                            <Input.TextArea
                                onChange={ev => this.updateComment(ev.target.value)}
                                value={this.state.comment}
                                maxLength={200} rows={4} />
                        </Form.Item>
                    </Form>
                }
            </Modal>
        );
    }
}

export default connect(
    (state: ApplicationState, ownProps: Props) => ({
        ...state.changeRequests,
        ...ownProps
    }), // Selects which state properties are merged into the component's props
    dispatch => bindActionCreators(changeRequestActionCreators, dispatch) // Selects which action creators are merged into the component's props
)(ChangeRequestModal as any);
