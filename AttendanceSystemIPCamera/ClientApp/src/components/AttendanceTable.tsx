import * as React from 'react';
import Unit from '../models/Unit';
import { Table } from 'antd';
import { unitActionCreators } from '../store/unit/actionCreators';
import { ColumnProps } from 'antd/lib/table';
import { recordActionCreators } from '../store/record/recordActionCreators';
import AttendanceRow from '../models/AttendanceTableRow';

export class AttendanceTable extends Table<AttendanceRow>{

}

export class AttendanceColumn extends Table.Column<AttendanceRow>{

}
