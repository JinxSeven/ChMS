import {
  type RouteConfig,
  index,
  route,
} from "@react-router/dev/routes";

export default [
  index("routes/home.tsx"),
  route("signin", "routes/auth/signin.tsx"),
  route("signup", "routes/auth/signup.tsx"),
] satisfies RouteConfig;
