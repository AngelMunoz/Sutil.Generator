import Navigo from 'navigo';

export const Router = new Navigo("/", { hash: true });

export function getCurrentLocation() {
    return Router
        .getCurrentLocation()
        .hashString
        .split("/")
        .filter(s => s);
}