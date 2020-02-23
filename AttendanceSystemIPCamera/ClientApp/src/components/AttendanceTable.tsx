import * as React from 'react';
import Unit from '../models/Unit';
import { Table } from 'antd';
import { unitActionCreators } from '../store/unit/actionCreators';
import { ColumnProps } from 'antd/lib/table';
import { recordActionCreators } from '../store/record/recordActionCreators';


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

class DataSource {
    key = 0;
    sunday = '';
    monday = '';
    tuesday = '';
    wednesday = '';
    thursday = '';
    friday = '';
    saturday = '';
    unit = '';
}

interface AttendanceTableState {
    columns: Column[];
    units: Unit[];
    events: Event[];
}

interface AttendanceTableProps {
    columns: Column[];
    units: Unit[];
    events: Event[];
}



export default class AttendanceTable extends React.Component<AttendanceTableProps, AttendanceTableState> {
    readonly state: AttendanceTableState;

    constructor(props: AttendanceTableProps) {
        super(props);
        this.state = {
            columns: this.props.columns,
            events: this.props.events,
            units: this.props.units
        };
    }

    render = () => {
        return (<>
            <Table columns={this.renderColumns()}
                dataSource={this.renderDataSource()}
                bordered />
        </>);
    }

    private renderColor(text: string, record: any, index: number) {
        return {
            props: {
                style: { background: text.includes("Present") ? "green" : text.includes('Absent') ? "red" : "" }
            },
            children: <div>{text}</div>
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
        switch (event.col) {
            case 'sunday':
                data.sunday = event.title;
                break;
            case 'monday':
                data.monday = event.title;
                break;
            case 'tuesday':
                data.tuesday = event.title;
                break;
            case 'wednesday':
                data.wednesday = event.title;
                break;
            case 'thursday':
                data.thursday = event.title;
                break;
            case 'friday':
                data.friday = event.title;
                break;
            case 'saturday':
                data.saturday = event.title;
                break;
        }
        return data;
    }



}

