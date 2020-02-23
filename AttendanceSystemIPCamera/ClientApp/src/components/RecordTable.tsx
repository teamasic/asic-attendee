

import * as React from 'react';
import Unit from '../models/Unit';
import { Table } from 'antd';
import { unitActionCreators } from '../store/unit/actionCreators';
import { ColumnProps } from 'antd/lib/table';
import Record from '../models/Record';


export class RecordTable extends Table<Record>{
    
}

export class DateOfWeekColumn extends Table.Column<Record>{

}

