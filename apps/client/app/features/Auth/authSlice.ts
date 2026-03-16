import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { signInUser, signUpUser } from "../../api/auth.api";
import type {
  AuthState,
  SignInRequest,
  SignUpRequest,
} from "../../interfaces/auth.interface";

const initialState: AuthState = {
  user: null,
  loading: false,
  error: null,
};

export const signIn = createAsyncThunk(
  "auth/signin",
  async (data: SignInRequest, thunkAPI) => {
    try {
      const loggedUser = await signInUser(data);
      return loggedUser;
    } catch (error: any) {
      return thunkAPI.rejectWithValue(
        error.response?.data?.message || "Login failed",
      );
    }
  },
);

export const signUp = createAsyncThunk(
  "auth/signup",
  async (data: SignUpRequest, thunkAPI) => {
    try {
      const response = await signUpUser(data);
      return response;
    } catch (error: any) {
      return thunkAPI.rejectWithValue(
        error.response?.data?.message || "Signup failed",
      );
    }
  },
);

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    logout(state) {
      state.user = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // SignIn Reducers
      .addCase(signIn.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(signIn.fulfilled, (state, action) => {
        state.loading = false;
        state.user = action.payload;
      })
      .addCase(signIn.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })

      // SignUp Reducers
      .addCase(signUp.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(signUp.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(signUp.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const { logout } = authSlice.actions;
export default authSlice.reducer;
