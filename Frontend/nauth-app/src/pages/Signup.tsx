import { useContext, useEffect, useState } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Checkbox } from "@/components/ui/checkbox"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { CodeExample } from "@/components/CodeExample"
import { UserPlus } from "lucide-react";
import reactCode from "../assets/code-examples/signup-react.txt?raw";
import dotnetCode from "../assets/code-examples/signup-dotnet.txt?raw";
import { IUserProvider, UserContext } from "@/lib/nauth-core";
import { toast } from "sonner";

export default function Signup() {


  const [insertMode, setInsertMode] = useState<boolean>(false);
  //const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")

  const userContext = useContext<IUserProvider>(UserContext);

  useEffect(() => {
    userContext.loadUserSession().then((ret) => {
      if (!ret.sucesso) {
        toast.error(ret.mensagemErro);
        return;
      }
      if (userContext.sessionInfo && userContext.sessionInfo?.userId > 0) {
        userContext.getMe().then((ret) => {
          console.log(ret);
          if (!ret.sucesso) {
            setInsertMode(true);
            return;
          }
          setInsertMode(false);
        });
      }
      else {
        setInsertMode(true);
      }
    });
  }, []);

  return (
    <div className="container mx-auto max-w-6xl py-8 px-4">
      <div className="mb-8">
        <h1 className="text-3xl font-bold tracking-tight mb-2">Simple Signup</h1>
        <p className="text-muted-foreground">
          Quick and easy user registration with essential fields
        </p>
      </div>

      <div className="grid gap-8 lg:grid-cols-2">
        {/* Signup Form */}
        <div>
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <UserPlus className="h-5 w-5" />
                Create Account
              </CardTitle>
              <CardDescription>
                Sign up for a new account with just email and password
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="email">Email</Label>
                <Input
                  id="email"
                  type="email"
                  placeholder="john@example.com"
                  value={userContext.user?.email}
                  onChange={(e) => {
                    userContext.setUser({
                      ...userContext.user,
                      email: e.target.value
                    });
                  }}
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
              <div className="space-y-2">
                <Label htmlFor="confirm-password">Confirm Password</Label>
                <Input
                  id="confirm-password"
                  type="password"
                  placeholder="••••••••"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                />
              </div>
              <Button
                className="w-full bg-gradient-to-r from-brand-primary to-brand-secondary hover:shadow-brand transition-all duration-300"
                onClick={async (e) => {
                  if (insertMode) {
                    if (userContext.user?.password != confirmPassword) {
                      toast.error("Passwords do not match!");
                      return;
                    }
                    let ret = await userContext.insert(userContext.user);
                    if (ret.sucesso) {
                      toast.success(ret.mensagemSucesso);
                    }
                    else {
                      toast.error(ret.mensagemErro);
                    }
                  }
                  else {
                    let ret = await userContext.update(userContext.user);
                    if (ret.sucesso) {
                      toast.success(ret.mensagemSucesso);
                    }
                    else {
                      toast.error(ret.mensagemErro);
                    }
                  }
                }}>
                Create Account
              </Button>
            </CardContent>
          </Card>
        </div>

        {/* Code Examples */}
        <div>
          <CodeExample
            reactCode={reactCode}
            dotnetCode={dotnetCode}
            title="Simple Registration Implementation"
          />
        </div>
      </div>
    </div>
  )
}