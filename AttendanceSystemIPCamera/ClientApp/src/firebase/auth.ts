import { auth } from "./firebase";
import * as firebase from "firebase/app";


export const doSignInWithGooogle = () => {
    const googleProvider = new firebase.auth.GoogleAuthProvider();
    return auth.signInWithPopup(googleProvider);
}

export const doSignOut = () => auth.signOut();

export const onAuthStateChanged = (nextOrObserver:
    | firebase.Observer<any>
    | ((a: firebase.User | null) => any),
    error?: (a: firebase.auth.Error) => any,
    completed?: firebase.Unsubscribe) => {
    auth.onAuthStateChanged(nextOrObserver, error, completed);
};

// Sign Up
// export const doCreateUserWithEmailAndPassword = (
//   email: string,
//   password: string
// ) => auth.createUserWithEmailAndPassword(email, password);

// // Sign In
// export const doSignInWithEmailAndPassword = (email: string, password: string) =>
//   auth.signInWithEmailAndPassword(email, password);


// // Password Reset
// export const doPasswordReset = (email: string) =>
//   auth.sendPasswordResetEmail(email);

// // Password Change
// export const doPasswordUpdate = async (password: string) => {
//   if (auth.currentUser) {
//     await auth.currentUser.updatePassword(password);
//   }
//   throw Error("No auth.currentUser!");
// };

