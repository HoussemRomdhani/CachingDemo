import http from "k6/http";
import { check, sleep } from "k6";

export const options = {
    thresholds: {
        // Assert that 99% of requests finish within 3000ms.
        http_req_duration: ["p(99) < 3000"],
    },
    // Ramp the number of virtual users up and down
    stages: [
        { duration: "10s", target: 50 },
    ],
};

// Simulated user behavior
export default function () {
    let res = http.get("http://localhost:5000/weatherForecast?city=Paris");
    check(res, { "status was 200": (r) => r.status == 200 });
}