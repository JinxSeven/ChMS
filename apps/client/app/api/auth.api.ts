import type {
  SignInRequest,
  SignUpRequest,
  User,
} from "~/interfaces/auth.interface";
import API from "./axios";

const RESOURCE_NAME = "auth";

export async function signInUser(data: SignInRequest): Promise<User> {
  return (await API.post<User>(`/${RESOURCE_NAME}/login`, data)).data;
}

export async function signUpUser(data: SignUpRequest): Promise<Partial<User>> {
  return (await API.post<Partial<User>>(`/${RESOURCE_NAME}/signup`, data)).data;
}
