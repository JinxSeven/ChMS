import type { Route } from "../+types/home";
import Signin from "../../pages/Signin";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "Welcome Back" },
    { name: "description", content: "Welcome to React Router!" },
  ];
}

export default function Home() {
  return <Signin />;
}
