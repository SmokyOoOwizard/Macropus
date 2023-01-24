import {createSlice, PayloadAction} from '@reduxjs/toolkit'

interface AppState {
    apiToken?: string;
}

const initialState: AppState = {}

export const appSlice = createSlice({
    name: 'app',
    initialState,
    reducers: {
        SetApiToken: (state: AppState, action: PayloadAction<string | undefined>) => {
            state.apiToken = action.payload
        }
    }
});

export const {SetApiToken} = appSlice.actions;

export default appSlice.reducer;