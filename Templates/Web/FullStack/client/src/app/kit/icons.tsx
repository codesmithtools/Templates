// Icons: inline SVG, 24×24, drawn with round strokes (no icon font, nothing fetched). A table's icon is one of these by
// name (the generated definition's icon); an unknown name is the table icon.
import type { SVGProps } from "react";

const paths: Record<string, string[]> = {
  search: ["M11 18a7 7 0 1 0 0-14 7 7 0 0 0 0 14Z", "m20 20-3.6-3.6"],
  plus: ["M12 5v14", "M5 12h14"],
  x: ["M18 6 6 18", "m6 6 12 12"],
  check: ["M20 6 9 17l-5-5"],
  "chevron-right": ["m9 18 6-6-6-6"],
  "chevron-left": ["m15 18-6-6 6-6"],
  "chevron-down": ["m6 9 6 6 6-6"],
  "chevron-up": ["m18 15-6-6-6 6"],
  "chevrons-left": ["m11 17-5-5 5-5", "m18 17-5-5 5-5"],
  "chevrons-right": ["m6 17 5-5-5-5", "m13 17 5-5-5-5"],
  "chevrons-up-down": ["m7 15 5 5 5-5", "m7 9 5-5 5 5"],
  "arrow-up": ["M12 19V5", "m5 12 7-7 7 7"],
  "arrow-down": ["M12 5v14", "m19 12-7 7-7-7"],
  "arrow-left": ["M19 12H5", "m12 19-7-7 7-7"],
  "arrow-right": ["M5 12h14", "m12 5 7 7-7 7"],
  "arrow-up-right": ["M7 17 17 7", "M8 7h9v9"],
  pencil: ["M16.5 3.5a2.1 2.1 0 0 1 3 3L8 18l-4 1 1-4Z", "m14.5 5.5 3 3"],
  trash: ["M3 6h18", "M8 6V4.5A1.5 1.5 0 0 1 9.5 3h5A1.5 1.5 0 0 1 16 4.5V6", "M18.5 6l-.8 13a2 2 0 0 1-2 2H8.3a2 2 0 0 1-2-2L5.5 6", "M10 11v6", "M14 11v6"],
  "alert-circle": ["M12 22a10 10 0 1 0 0-20 10 10 0 0 0 0 20Z", "M12 8v4.5", "M12 16h.01"],
  "alert-triangle": ["M10.3 3.9 2.2 18a2 2 0 0 0 1.7 3h16.2a2 2 0 0 0 1.7-3L13.7 3.9a2 2 0 0 0-3.4 0Z", "M12 9v4", "M12 17h.01"],
  info: ["M12 22a10 10 0 1 0 0-20 10 10 0 0 0 0 20Z", "M12 16v-4.5", "M12 8h.01"],
  "check-circle": ["M12 22a10 10 0 1 0 0-20 10 10 0 0 0 0 20Z", "m8.5 12 2.5 2.5 4.5-5"],
  "x-circle": ["M12 22a10 10 0 1 0 0-20 10 10 0 0 0 0 20Z", "m15 9-6 6", "m9 9 6 6"],
  sun: ["M12 16a4 4 0 1 0 0-8 4 4 0 0 0 0 8Z", "M12 2v2", "M12 20v2", "m4.9 4.9 1.4 1.4", "m17.7 17.7 1.4 1.4", "M2 12h2", "M20 12h2", "m6.3 17.7-1.4 1.4", "m19.1 4.9-1.4 1.4"],
  moon: ["M20.5 14.2A8.5 8.5 0 1 1 9.8 3.5a6.6 6.6 0 0 0 10.7 10.7Z"],
  monitor: ["M4 4h16a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2Z", "M8 21h8", "M12 17v4"],
  grid: ["M4 3h6a1 1 0 0 1 1 1v6a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1Z", "M14 3h6a1 1 0 0 1 1 1v6a1 1 0 0 1-1 1h-6a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1Z", "M14 13h6a1 1 0 0 1 1 1v6a1 1 0 0 1-1 1h-6a1 1 0 0 1-1-1v-6a1 1 0 0 1 1-1Z", "M4 13h6a1 1 0 0 1 1 1v6a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1v-6a1 1 0 0 1 1-1Z"],
  database: ["M12 8c5 0 9-1.3 9-3s-4-3-9-3-9 1.3-9 3 4 3 9 3Z", "M3 5v14c0 1.7 4 3 9 3s9-1.3 9-3V5", "M3 12c0 1.7 4 3 9 3s9-1.3 9-3"],
  table: ["M5 3h14a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2Z", "M3 9h18", "M3 15h18", "M9 9v12"],
  user: ["M12 12a4.5 4.5 0 1 0 0-9 4.5 4.5 0 0 0 0 9Z", "M4 21a8 8 0 0 1 16 0"],
  shield: ["M12 21.5s8-3.6 8-10V5.3L12 2.5 4 5.3v6.2c0 6.4 8 10 8 10Z"],
  receipt: ["M5 2.5h14v19l-2.3-1.5-2.4 1.5-2.3-1.5-2.3 1.5L7.3 20 5 21.5Z", "M9 7.5h6", "M9 11.5h6", "M9 15.5h3.5"],
  cart: ["M3 3h2.2l2.3 11.3a2 2 0 0 0 2 1.7h8.3a2 2 0 0 0 2-1.6L21.2 7H6", "M10 21a1 1 0 1 0 0-2 1 1 0 0 0 0 2Z", "M18 21a1 1 0 1 0 0-2 1 1 0 0 0 0 2Z"],
  box: ["m21 7.5-9-5-9 5v9l9 5 9-5Z", "m3 7.5 9 5 9-5", "M12 12.5v9"],
  package: ["m21 7.5-9-5-9 5v9l9 5 9-5Z", "m3 7.5 9 5 9-5", "M12 12.5v9", "m7.5 5 9 5"],
  tag: ["M3 4v7.2a2 2 0 0 0 .6 1.4l8.8 8.8a2 2 0 0 0 2.8 0l6.2-6.2a2 2 0 0 0 0-2.8L12.6 3.6A2 2 0 0 0 11.2 3H4a1 1 0 0 0-1 1Z", "M7.5 7.5h.01"],
  truck: ["M2 5h12v11H2Z", "M14 9h4l4 4v3h-8", "M6.5 20a2 2 0 1 0 0-4 2 2 0 0 0 0 4Z", "M17.5 20a2 2 0 1 0 0-4 2 2 0 0 0 0 4Z"],
  layers: ["m12 2.5 10 5-10 5-10-5Z", "m2 12 10 5 10-5", "m2 16.5 10 5 10-5"],
  list: ["M8 6h13", "M8 12h13", "M8 18h13", "M3.5 6h.01", "M3.5 12h.01", "M3.5 18h.01"],
  activity: ["M22 12h-4l-3 8-6-16-3 8H2"],
  "map-pin": ["M20 10c0 6-8 12-8 12s-8-6-8-12a8 8 0 0 1 16 0Z", "M12 13a3 3 0 1 0 0-6 3 3 0 0 0 0 6Z"],
  "credit-card": ["M4 5h16a2 2 0 0 1 2 2v10a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V7a2 2 0 0 1 2-2Z", "M2 10h20", "M6 15h4"],
  building: ["M6 2h12a2 2 0 0 1 2 2v18H4V4a2 2 0 0 1 2-2Z", "M10 22v-4h4v4", "M9 6h.01", "M15 6h.01", "M12 6h.01", "M9 10h.01", "M15 10h.01", "M12 10h.01", "M9 14h.01", "M15 14h.01", "M12 14h.01"],
  message: ["M21 15a2 2 0 0 1-2 2H8l-5 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2Z"],
  settings: ["M4 21v-7", "M4 10V3", "M12 21v-9", "M12 8V3", "M20 21v-5", "M20 12V3", "M2 14h4", "M10 8h4", "M18 16h4"],
  calendar: ["M5 4h14a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2Z", "M16 2v4", "M8 2v4", "M3 10h18"],
  file: ["M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8Z", "M14 2v6h6"],
  link: ["M10 13a5 5 0 0 0 7.5.5l3-3a5 5 0 0 0-7-7l-1.7 1.7", "M14 11a5 5 0 0 0-7.5-.5l-3 3a5 5 0 0 0 7 7l1.7-1.7"],
  "external-link": ["M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6", "M15 3h6v6", "M10 14 21 3"],
  key: ["M7.5 21a5.5 5.5 0 1 0 0-11 5.5 5.5 0 0 0 0 11Z", "m11.4 11.6 9.1-9.1", "m16 7 3 3", "m19 4 2.5 2.5"],
  hash: ["M4 9h16", "M4 15h16", "M10 3 8 21", "m16 3-2 18"],
  mail: ["M4 4h16a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2Z", "m22 7-10 6.5L2 7"],
  phone: ["M22 16.9v3a2 2 0 0 1-2.2 2 19.8 19.8 0 0 1-8.6-3.1 19.5 19.5 0 0 1-6-6A19.8 19.8 0 0 1 2.1 4.2 2 2 0 0 1 4.1 2h3a2 2 0 0 1 2 1.7c.1.9.4 1.9.7 2.8a2 2 0 0 1-.5 2.1L8.1 9.9a16 16 0 0 0 6 6l1.3-1.3a2 2 0 0 1 2.1-.4c.9.3 1.9.6 2.8.7a2 2 0 0 1 1.7 2Z"],
  globe: ["M12 22a10 10 0 1 0 0-20 10 10 0 0 0 0 20Z", "M2 12h20", "M12 2a15 15 0 0 1 4 10 15 15 0 0 1-4 10 15 15 0 0 1-4-10 15 15 0 0 1 4-10Z"],
  lock: ["M5 11h14a2 2 0 0 1 2 2v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-7a2 2 0 0 1 2-2Z", "M7 11V7a5 5 0 0 1 10 0v4"],
  refresh: ["M21 12a9 9 0 1 1-2.6-6.4L21 8", "M21 3v5h-5"],
  more: ["M5 12h.01", "M12 12h.01", "M19 12h.01"],
  menu: ["M4 6h16", "M4 12h16", "M4 18h16"],
  command: ["M15 6v12a3 3 0 1 0 3-3H6a3 3 0 1 0 3 3V6a3 3 0 1 0-3 3h12a3 3 0 1 0-3-3"],
  inbox: ["M22 12h-6l-2 3h-4l-2-3H2", "M5.5 5.1 2 12v6a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-6l-3.5-6.9A2 2 0 0 0 16.8 4H7.2a2 2 0 0 0-1.7 1.1Z"],
  filter: ["M22 3H2l8 9.5V19l4 2v-8.5Z"],
  copy: ["M11 9h9a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2h-9a2 2 0 0 1-2-2v-9a2 2 0 0 1 2-2Z", "M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"],
  keyboard: ["M4 5h16a2 2 0 0 1 2 2v10a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V7a2 2 0 0 1 2-2Z", "M6 9h.01", "M10 9h.01", "M14 9h.01", "M18 9h.01", "M8 13h.01", "M12 13h.01", "M16 13h.01", "M7 16h10"],
  enter: ["m9 10-5 5 5 5", "M20 4v7a4 4 0 0 1-4 4H4"],
  clock: ["M12 22a10 10 0 1 0 0-20 10 10 0 0 0 0 20Z", "M12 6v6l4 2"],
  type: ["M4 7V4h16v3", "M9 20h6", "M12 4v16"],
  code: ["m16 18 6-6-6-6", "m8 6-6 6 6 6"],
  branch: ["M6 3v12", "M18 9a3 3 0 1 0 0-6 3 3 0 0 0 0 6Z", "M6 21a3 3 0 1 0 0-6 3 3 0 0 0 0 6Z", "M18 9a9 9 0 0 1-9 9"],
  rows: ["M4 4h16a1 1 0 0 1 1 1v4a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V5a1 1 0 0 1 1-1Z", "M4 14h16a1 1 0 0 1 1 1v4a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1v-4a1 1 0 0 1 1-1Z"],
  columns: ["M5 3h14a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2Z", "M12 3v18"],
  sparkle: ["M12 3l1.9 5.1L19 10l-5.1 1.9L12 17l-1.9-5.1L5 10l5.1-1.9Z", "M19 3v4", "M21 5h-4"],
  "toggle-on": ["M8 6h8a6 6 0 0 1 0 12H8A6 6 0 0 1 8 6Z", "M16 15a3 3 0 1 0 0-6 3 3 0 0 0 0 6Z"],
  minus: ["M5 12h14"],
};

export type IconName = keyof typeof paths;

export interface IconProps extends Omit<SVGProps<SVGSVGElement>, "name"> {
  name: string;
  size?: number;
  strokeWidth?: number;
  /** Its text for a screen reader; without, it's decoration. */
  label?: string;
}

export function Icon({ name, size = 16, strokeWidth = 1.75, label, ...rest }: IconProps) {
  const d = paths[name] ?? paths.table;
  return (
    <svg
      width={size}
      height={size}
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth={strokeWidth}
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden={label ? undefined : true}
      role={label ? "img" : undefined}
      aria-label={label}
      focusable="false"
      {...rest}
    >
      {d.map((path) => <path key={path} d={path} />)}
    </svg>
  );
}
