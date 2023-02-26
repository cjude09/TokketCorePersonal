import axios from 'axios';

const apiUrl = process.env.REACT_APP_API_URL;
const apiVersion = process.env.REACT_APP_API_VERSION;
const apiCode = process.env.REACT_APP_API_CODE;

export async function getClassToks() {
    return await axios.get(`${apiUrl}${apiVersion}/classtoks?code=${apiCode}`);
}