import * as Moment from 'moment';
import { extendMoment } from 'moment-range';


const moment = extendMoment(Moment);

export default class Unit {
    id = 0;
    name = "";
    startTime = Date();
    endTime = Date();
    public getUnitLabel() {
        return moment(this.startTime).format("HH:mm") + " - " + moment(this.endTime).format("HH:mm");
    }

}