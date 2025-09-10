import React from 'react';
import IUserProvider from './IUserProvider';

const UserContext = React.createContext<IUserProvider>(null as unknown as IUserProvider);

export default UserContext;
