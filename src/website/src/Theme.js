import { parseColorString, createColorPalette } from '@microsoft/fast-components';
const prefersDarkQuery = window.matchMedia('(prefers-color-scheme: dark)');
const isDark = () => prefersDarkQuery.matches;
export function registerThemeEventListener(cb) {
    cb(isDark());
    prefersDarkQuery.addEventListener('change', () => cb(isDark()));
}

export function getPalette(color) {
    return createColorPalette(parseColorString(color));
}