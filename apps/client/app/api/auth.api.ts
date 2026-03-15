import type { SignUpRequest, User } from "~/interfaces/auth.interface";
import API from "./axios";

const RESOURCE_NAME = "auth";

export const signInUser = (data: { email: string; password: string }) =>
  API.post(`/${RESOURCE_NAME}/login`, data);

// TODO: Return strongly typed response
export const signUpUser = (data: SignUpRequest) =>
  API.post(`/${RESOURCE_NAME}/signup`, data);
