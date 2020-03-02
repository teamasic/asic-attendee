import * as React from 'react';
import Unit from '../models/Unit';
import Record from '../models/Record';
import { Table, Button, Modal, Form, Input, Icon } from 'antd';
import { unitActionCreators } from '../store/unit/actionCreators';
import { ColumnProps } from 'antd/lib/table';
import { recordActionCreators } from '../store/record/recordActionCreators';
import { MinusCircleOutline } from '@ant-design/icons';
import ChangeRequestModal from './ChangeRequestModal';
import classNames from 'classnames';

const initState = {
    columns: [{
        id: "sunday",
        name: "20 Freb"
    }, {
        id: "monday",
        name: "20 Freb"
    }, {
        id: "tuesday",
        name: "20 Freb"
    }, {
        id: "wednesday",
        name: "20 Freb"
    }, {
        id: "thursday",
        name: "20 Freb"
    }, {
        id: "friday",
        name: "20 Freb"
    }, {
        id: "saturday",
        name: "20 Freb"
    }],
    units: [{
        id: 1,
        name: "Slot 1",
        startDate: new Date(),
        endDate: new Date()
    }, {
        id: 2,
        name: "Slot 2",
        startDate: new Date(),
        endDate: new Date()
    }, {
        id: 3,
        name: "Slot 3",
        startDate: new Date(),
        endDate: new Date()
    }, {
        id: 4,
        name: "Slot 4",
        startDate: new Date(),
        endDate: new Date()
    }],
    events: [{
        id: 123,
        title: "DBI202-Present",
        col: "monday",
        row: 2,
        present: true,
        date: ""
    }]

};

export interface Event {
    id: number,
    title: string,
    col: string,
    row: number,
    present: boolean,
    date: string,
}

interface Column {
    id: string,
    name: string
}

class Cell {
    id = 0;
    text = '';
}

class DataSource {
    key = 0;
    sunday = new Cell();
    monday = new Cell();
    tuesday = new Cell();
    wednesday = new Cell();
    thursday = new Cell();
    friday = new Cell();
    saturday = new Cell();
    unit = '';
}

interface AttendanceTableState {
    columns: Column[];
    units: Unit[];
    events: Event[];
    modalVisible: boolean;
    activeRecord?: Record;
}

interface AttendanceTableProps {
    columns: Column[];
    units: Unit[];
    events: Event[];
    records: Record[];
}

export default class AttendanceTable extends React.Component<AttendanceTableProps, AttendanceTableState> {
    readonly state: AttendanceTableState;

    constructor(props: AttendanceTableProps) {
        super(props);
        this.state = {
            columns: this.props.columns,
            events: this.props.events,
            units: this.props.units,
            modalVisible: false
        };
    }

    render = () => {
        return (<>
            <ChangeRequestModal
                visible={this.state.modalVisible}
                record={this.state.activeRecord}
                hideModal={() => this.hideModal()}
            />
            <Table columns={this.renderColumns()}
                dataSource={this.renderDataSource()}
                bordered />
        </>);
    }

    private showModal(id: number) {
        this.setState({
            modalVisible: true,
            activeRecord: this.props.records.find(r => r.id === id)
        });
    }

    private hideModal() {
        this.setState({
            modalVisible: false
        });
    }

    private renderColor(cell: Cell, record: any, index: number) {
        const hasRecord = cell.id > 0;
        const isAbsent = cell.text.includes("Absent");
        return {
            props: {
                className: classNames("table-cell", {
                    'is-absent': hasRecord && isAbsent,
                    'is-present': hasRecord && !isAbsent
                }),
                /*
                style: {
                    background: cell.text.includes("Present") ? "#3BDE86" :
                        cell.text.includes('Absent') ? "#FF6260" : ""
                },
                */
                onClick: () => {
                    if (hasRecord && isAbsent) {
                        this.showModal(cell.id);
                    }
                }
            },
            children: <>
                {
                    (hasRecord && isAbsent) && <Icon type="question-circle" />
                }
                <div className="table-cell-text">
                    {cell.text}
                </div>
            </>
        };
    }

    private renderColumns() {
        const columns: ColumnProps<any>[] = [];
        columns.push({
            title: '',
            dataIndex: "unit",
            key: "unit",
            align: 'center',
        });

        const cols = this.state.columns.map((col, i) => {
            return {
                title: col.name,
                dataIndex: col.id,
                key: col.id,
                align: 'center',
                render: (text, record, index) => this.renderColor(text, record, index)
            } as ColumnProps<{}>;
        })

        columns.push(...cols);
        return columns;
    }

    private renderDataSource() {
        const datasource: DataSource[] = [];

        this.state.units.forEach((unit, i) => {
            const ds = new DataSource();
            ds.key = unit.id
            ds.unit = unit.name
            const events = this.state.events.filter(e => e.row == unit.id);
            events.forEach(e => this.renderEvent(e, ds))
            datasource.push(ds);
        })
        return datasource;
    }

    private renderEvent(event: Event, data: DataSource) {
        const ev = {
            id: event.id,
            text: event.title
        };
        switch (event.col) {
            case 'sunday':
                data.sunday = ev;
                break;
            case 'monday':
                data.monday = ev;
                break;
            case 'tuesday':
                data.tuesday = ev;
                break;
            case 'wednesday':
                data.wednesday = ev;
                break;
            case 'thursday':
                data.thursday = ev;
                break;
            case 'friday':
                data.friday = ev;
                break;
            case 'saturday':
                data.saturday = ev;
                break;
        }
        return data;
    }



}

