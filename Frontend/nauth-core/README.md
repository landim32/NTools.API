# NAuth Core

Reusable React components, contexts and services used by the NAuth project.
This package provides the frontend building blocks for authentication flows,
including API clients and React context providers.

## Installation

```bash
npm install nauth-core
```

## Usage

After installing, import the providers and components you need:

```tsx
import { AuthProvider, UserProvider, MessageToast } from 'nauth-core';
```

Run `npm run build` to compile the TypeScript sources before publishing.

## Publishing

Use `npm publish` to publish the contents of the `dist` folder to npm
(after running `npm run build`).
