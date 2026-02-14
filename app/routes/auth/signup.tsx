import type { Route } from "../+types/home";
import Signup from "../../pages/Signup";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "Create Your Account" },
    { name: "description", content: "Welcome to React Router!" },
  ];
}

export default function Home() {
  return <Signup />;
}
