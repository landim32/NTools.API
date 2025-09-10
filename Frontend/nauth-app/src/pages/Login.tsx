import { useContext, useState } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { CodeExample } from "@/components/CodeExample"
import { LogIn } from "lucide-react"
import { IUserProvider, UserContext } from "@/lib/nauth-core";
import { toast } from "sonner";
import reactCode from "../assets/code-examples/login-react.txt?raw";
import dotnetCode from "../assets/code-examples/login-dotnet.txt?raw";

export default function Login() {
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")

  const userContext = useContext<IUserProvider>(UserContext);

  return (
    <div className="container mx-auto max-w-6xl py-8 px-4">
      <div className="mb-8">
        <h1 className="text-3xl font-bold tracking-tight mb-2">Login</h1>
        <p className="text-muted-foreground">
          Secure user authentication with customizable login forms
        </p>
      </div>

      <div className="grid gap-8 lg:grid-cols-2">
        {/* Login Form */}
        <div>
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <LogIn className="h-5 w-5" />
                User Login
              </CardTitle>
              <CardDescription>
                Enter your credentials to access your account
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="email">Email</Label>
                <Input
                  id="email"
                  type="email"
                  placeholder="john@example.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="password">Password</Label>
                <Input
                  id="password"
                  type="password"
                  placeholder="••••••••"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
              </div>
              <Button className="w-full bg-gradient-to-r from-brand-primary to-brand-secondary hover:shadow-brand transition-all duration-300" 
                onClick={async (e) => {
                  e.preventDefault();
                  let ret = await userContext.loginWithEmail(email, password); 
                  if (ret.sucesso) {
                    toast.success("Logged in successfully!");
                  }
                  else {
                    toast.error(ret.mensagemErro);
                  }
                }}>
                {userContext.loading ? 'Signing in...' : 'Sign In'}
              </Button>
            </CardContent>
          </Card>
        </div>

        {/* Code Examples */}
        <div>
          <CodeExample
            reactCode={reactCode}
            dotnetCode={dotnetCode}
            title="Login Implementation"
          />
        </div>
      </div>
    </div>
  )
}