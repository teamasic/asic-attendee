import * as React from 'react';
import { Button, Icon } from 'antd';
import moment from 'moment';

const buttonGroupState = {
    // today: new Date()
}

const buttonGroupProps = {
    onNavigate: (date: Date) => {

    },
    today: new Date()
}

const WEEK = 1;

const ButtonGroup = Button.Group;
type ButtonGroupState = Readonly<typeof buttonGroupState>;
type ButtonGroupProps = Readonly<typeof buttonGroupProps>;


export default class AttendanceButtonGroup extends React.Component<ButtonGroupProps, ButtonGroupState> {

    state: ButtonGroupState = buttonGroupState;
    today = new Date();

    constructor(props: any) {
        super(props);
        this.today = this.props.today;
    }

    render() {
        return (<ButtonGroup className="record-button-group">
            <Button onClick={this.handleOnClick} id='previous'>
                <Icon type="caret-left" />
                Previous
                </Button>
            <Button onClick={this.handleOnClick} id='next'>
                <Icon type="caret-right" />
                Next</Button>
            <Button onClick={this.handleOnClick} id='today'>Today</Button>
        </ButtonGroup>)
    }

    handleOnClick = (e: React.MouseEvent) => {
        console.log(e.currentTarget.id)
        switch (e.currentTarget.id) {
            case 'previous':
                this.today = moment(this.today).subtract(1, "w").toDate();
                break;
            case 'next':
                this.today = moment(this.today).add(1, "w").toDate();
                break;
            case 'today':
                this.today = new Date();
                break;
        }
        console.log(this.today);
        this.props.onNavigate(this.today);
    }





}
