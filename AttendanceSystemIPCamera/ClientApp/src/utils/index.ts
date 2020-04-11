import format from 'date-fns/format';
import moment from 'moment';
import Swal from 'sweetalert2'

export const formatFullDateTimeString = (time: Date | string) => format(new Date(time), 'EEE, dd/MM/yyyy');
export const minutesOfDay = (time: moment.Moment) => {
    return time.minutes() + time.hours() * 60;
}
export const formatDateString = (time: Date | string) => format(new Date(time), 'dd/MM/yyyy');



export const renderStripedTable = (record: any, index: number) => {
    if (index % 2 === 0) {
        return 'default';
    } else {
        return 'striped';
    }
}

export const success = (msg: string) => {
    Swal.fire({
        icon: 'success',
        text: msg
    });
}

export const error = (msg: string) => {
    Swal.fire({
        icon: 'error',
        text: msg
    });
}

export const warning = (msg: string) => {
    Swal.fire({
        icon: 'warning',
        text: msg
    });
}

export const getErrors = (errors: any[])=>{
    const values = []
    for(const key in errors){
        values.push(errors[key]);
    }
    return values.toString();
}