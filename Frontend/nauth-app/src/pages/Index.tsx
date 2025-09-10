import { Link } from "react-router-dom"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Shield, Code, Zap, Users, Lock, Globe } from "lucide-react"
import nauthLogo from "@/assets/nauth-logo.png"

const features = [
  {
    icon: Shield,
    title: "Secure by Design",
    description: "Built-in security best practices with JWT tokens, password hashing, and rate limiting"
  },
  {
    icon: Code,
    title: "Developer Friendly",
    description: "Clean APIs, comprehensive documentation, and ready-to-use code examples"
  },
  {
    icon: Zap,
    title: "Fast Integration",
    description: "Get authentication up and running in minutes with our simple SDKs"
  },
  {
    icon: Users,
    title: "User Management",
    description: "Complete user lifecycle management with profiles, roles, and permissions"
  },
  {
    icon: Lock,
    title: "Enterprise Ready",
    description: "GDPR compliant, SOC 2 certified, and enterprise-grade security"
  },
  {
    icon: Globe,
    title: "Multi-Platform",
    description: "Works with React, Angular, Vue, .NET, Node.js, and more"
  }
]

const demos = [
  {
    title: "Login",
    description: "Secure user authentication",
    href: "/login",
    badge: "Essential"
  },
  {
    title: "Simple Signup",
    description: "Quick registration form",
    href: "/signup",
    badge: "Basic"
  },
  {
    title: "Full Registration",
    description: "Complete user onboarding",
    href: "/register",
    badge: "Advanced"
  },
  {
    title: "Change Password",
    description: "Secure password updates",
    href: "/change-password",
    badge: "Security"
  },
  {
    title: "Recover Password",
    description: "Password reset flow",
    href: "/recover-password",
    badge: "Recovery"
  }
]

const Index = () => {
  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <section className="relative py-20 px-4 bg-gradient-to-br from-brand-light via-background to-brand-light/30">
        <div className="container mx-auto max-w-6xl text-center">
          <div className="flex justify-center mb-8">
            <img src={nauthLogo} alt="NAuth Logo" className="h-20 w-20" />
          </div>
          <h1 className="text-5xl md:text-7xl font-bold tracking-tight mb-6">
            <span className="bg-gradient-to-r from-brand-primary to-brand-secondary bg-clip-text text-transparent">
              NAuth
            </span>
          </h1>
          <p className="text-xl md:text-2xl text-muted-foreground mb-4 max-w-3xl mx-auto">
            Complete and secure user authentication for your SaaS or web platform
          </p>
          <p className="text-lg text-muted-foreground mb-8 max-w-2xl mx-auto">
            Built with .NET Core and React, NAuth provides everything you need for modern authentication
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Button 
              size="lg" 
              asChild
              className="bg-gradient-to-r from-brand-primary to-brand-secondary hover:shadow-brand transition-all duration-300 text-lg px-8 py-6"
            >
              <Link to="/login">Try Live Demo</Link>
            </Button>
            <Button 
              size="lg" 
              variant="outline" 
              asChild
              className="text-lg px-8 py-6 border-brand-primary/20 hover:bg-brand-primary/10"
            >
              <a href="https://github.com/landim32/nauth" target="_blank" rel="noopener noreferrer">
                Visit your GitHub Repository
              </a>
            </Button>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-20 px-4">
        <div className="container mx-auto max-w-6xl">
          <div className="text-center mb-16">
            <h2 className="text-3xl md:text-4xl font-bold tracking-tight mb-4">
              Why Choose NAuth?
            </h2>
            <p className="text-lg text-muted-foreground max-w-2xl mx-auto">
              Everything you need for secure, scalable authentication in one powerful package
            </p>
          </div>
          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
            {features.map((feature) => (
              <Card key={feature.title} className="border-border/50 hover:border-brand-primary/50 transition-colors">
                <CardHeader>
                  <feature.icon className="h-10 w-10 text-brand-primary mb-2" />
                  <CardTitle className="text-xl">{feature.title}</CardTitle>
                </CardHeader>
                <CardContent>
                  <p className="text-muted-foreground">{feature.description}</p>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      {/* Demo Section */}
      <section className="py-20 px-4 bg-muted/30">
        <div className="container mx-auto max-w-6xl">
          <div className="text-center mb-16">
            <h2 className="text-3xl md:text-4xl font-bold tracking-tight mb-4">
              Interactive Demos
            </h2>
            <p className="text-lg text-muted-foreground max-w-2xl mx-auto">
              Explore our authentication components and see the implementation code
            </p>
          </div>
          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
            {demos.map((demo) => (
              <Card key={demo.title} className="group hover:shadow-lg transition-all duration-300 border-border/50 hover:border-brand-primary/50">
                <CardHeader>
                  <div className="flex items-center justify-between">
                    <CardTitle className="text-lg group-hover:text-brand-primary transition-colors">
                      {demo.title}
                    </CardTitle>
                    <Badge variant="secondary">{demo.badge}</Badge>
                  </div>
                  <CardDescription>{demo.description}</CardDescription>
                </CardHeader>
                <CardContent>
                  <Button asChild className="w-full group-hover:bg-brand-primary group-hover:text-white transition-colors">
                    <Link to={demo.href}>Try Demo</Link>
                  </Button>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-20 px-4">
        <div className="container mx-auto max-w-4xl text-center">
          <h2 className="text-3xl md:text-4xl font-bold tracking-tight mb-4">
            Ready to Get Started?
          </h2>
          <p className="text-lg text-muted-foreground mb-8 max-w-2xl mx-auto">
            Join to developers who trust NAuth for their authentication needs
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Button 
              size="lg" 
              asChild
              className="bg-gradient-to-r from-brand-primary to-brand-secondary hover:shadow-brand transition-all duration-300"
            >
              <a href="https://github.com/landim32/nauth" target="_blank" rel="noopener noreferrer">
                View on GitHub
              </a>
            </Button>
          </div>
        </div>
      </section>
    </div>
  )
};

export default Index;