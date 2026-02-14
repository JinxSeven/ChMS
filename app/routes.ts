import {
  type RouteConfig,
  index,
  layout,
  route,
} from "@react-router/dev/routes";

export default [
  index("routes/home.tsx"),
  layout("routes/auth/layout.tsx", [
      route("signin", "routes/auth/signin.tsx"),
      route("signup", "routes/auth/signup.tsx"),
  ]),
  // route("signin", "routes/signin.tsx"),
  // route("signup", "routes/signup.tsx"),
] satisfies RouteConfig;
