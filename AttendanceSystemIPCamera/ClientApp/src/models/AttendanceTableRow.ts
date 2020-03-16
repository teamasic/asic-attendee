import {Cell} from "../components/Record"

export default interface AttendanceTableRow {
    key: string,
    unit?: Cell, //unit label to display
    sunday?: Cell,
    monday?: Cell,
    tuesday?: Cell,
    wednesday?: Cell,
    thursday?: Cell,
    friday?: Cell,
    saturday?: Cell,
}